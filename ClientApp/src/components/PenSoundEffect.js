import React, { useEffect, useState } from 'react';
import writingSound from './pen_writing_1.mp3';

const PenSoundEffect = ({ text }) => {
    const [currentText, setCurrentText] = useState('');
    const [currentIndex, setCurrentIndex] = useState(0);
    const [audio] = useState(new Audio(writingSound));

    useEffect(() => {
        const timer = setInterval(() => {
            if (currentIndex < text.length) {
                setCurrentText((prevText) => prevText + text[currentIndex]);
                setCurrentIndex((prevIndex) => prevIndex + 1);
                audio.play();
            } else {
                audio.pause();
            }
        }, 20); // Adjust the typing speed by changing the interval time (e.g., 100 for 100ms)

        return () => {
            clearInterval(timer);
        };
    }, [currentIndex, text, audio]);

    return <div dangerouslySetInnerHTML={{ __html: currentText.replace(/\n/g, '<br/>') }}></div>;
};

export default PenSoundEffect;