import React, { Component } from 'react';
import PenSoundEffect from './PenSoundEffect';
import './Home.css';

export class Home extends Component {
    static displayName = Home.name;

    state = {
        imageUploaded: false,
        imageUrl: '',
        loading: false,
        textData: '',
        fileName: '',
    };

    handleImageUpload = async (event) => {
        const file = event.target.files[0];

        if (file && file.type.startsWith('image/')) {
            const reader = new FileReader();
            reader.onloadend = () => {
                const imageDataUrl = URL.createObjectURL(file);
                const fileName = file.name.split('.').slice(0, -1).join('.');
                this.setState({
                    imageUploaded: true,
                    imageUrl: imageDataUrl,
                    loading: true,
                    fileName: fileName,
                }, () => {
                    setTimeout(() => {
                        this.uploadImage(reader.result);
                    }, 1000);
                });
            };
            reader.readAsArrayBuffer(file);
        } else {
            console.error('Invalid file type. Please select an image.');
        }
    };

    uploadImage = async (imageData) => {
        try {
            const response = await fetch('poems/generate', {
                method: 'POST',
                body: imageData,
                headers: {
                    'Content-Type': 'application/octet-stream'
                }
            });

            if (response.ok) {
                const textData = await response.text();
                this.setState({ textData, loading: false });
            } else {
                console.error('Error uploading image');
                this.setState({ loading: false });
            }
        } catch (error) {
            console.error('Error uploading image', error);
            this.setState({ loading: false });
        }
    };

    render() {
        const { imageUploaded, imageUrl, loading, textData, fileName } = this.state;

        const imageViewerPage = (
            <div className={`image ${imageUploaded ? 'fade-in' : ''}`}>
                {imageUploaded && (
                    <div className="img-tape img-tape--1">
                        <div className="image-card">
                            <img src={imageUrl} alt="Uploaded" style={{ maxWidth: '100%' }} />
                            <div className="caption">{fileName}</div>
                        </div>
                    </div>
                )}
            </div>
        );

        const textViewerPage = (
            <div className="poem">
                {loading ? (
                    <p style={{ color: '#00000059' }}>Within this fleeting moment, I shall birth a universe of words... Please allow me to think...</p>
                ) : (
                    <div>
                        <PenSoundEffect text={textData.replace(/^\n\n/, '')} />
                    </div>
                )}
            </div>
        );

        return (
            <div>
                <div style={{ paddingTop: '50px', textAlign: 'center' }}>
                    <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
                        <img src={require('./logo2-zoomed.png')} alt="Logo Icon" style={{ height: '118px', width: '118px', top: '0px' }} />
                        <h1 className="title">POE<br />THE<br />POET</h1>
                    </div>
                </div>
                <div style={{ paddingTop: '30px', paddingBottom: '20px', textAlign: 'center' }}>
                    <label className="grow" htmlFor="upload" style={{
                        display: 'inline-block',
                        padding: '10px 20px',
                        color: 'black',
                        border: 'none',
                        borderRadius: '5px',
                        fontSize: '16px',
                        cursor: 'pointer',
                    }}>
                        <span className="image-upload-btn shiny-text">Inspire me with an image</span>
                        <img src={require('./quill-icon.png')} alt="Upload" style={{ marginLeft: '5px', height: '35px', width: '35px' }} />

                        <input
                            type="file"
                            id="upload"
                            accept="image/*"
                            onChange={this.handleImageUpload}
                            style={{
                                display: 'none',
                            }}
                        />
                    </label>
                </div>
                <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '10px' }}>
                    {imageViewerPage}
                    {textViewerPage}
                </div>
            </div>
        );
    }
}